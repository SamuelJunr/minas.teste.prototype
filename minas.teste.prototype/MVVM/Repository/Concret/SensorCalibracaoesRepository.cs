// minas.teste.prototype.MVVM.Repository.Concret.SensorCalibracaoRepository.cs
using System;
using System.Collections.Generic;
using System.Linq;
using minas.teste.prototype.MVVM.Model.Concrete;

namespace minas.teste.prototype.MVVM.Repository.Concret
{
    public class SensorCalibracaoRepository : Repository<SensorCalibracoes>
    {
        public SensorCalibracaoRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Métodos específicos para calibração, se necessário:

        /// <summary>
        /// Obtém o registro de calibração mais recente para um sensor específico.
        /// </summary>
        public SensorCalibracoes GetLatestBySensorId(string sensorIdentificador)
        {
            return _dbSet
                .Where(sc => sc.SensorIdentificador == sensorIdentificador)
                .OrderByDescending(sc => sc.DataCalibracao)
                .FirstOrDefault();
        }

        /// <summary>
        /// Obtém todos os registros de calibração para um sensor específico.
        /// </summary>
        public IEnumerable<SensorCalibracoes> GetAllBySensorId(string sensorIdentificador)
        {
            return Find(sc => sc.SensorIdentificador == sensorIdentificador)
                   .OrderByDescending(sc => sc.DataCalibracao)
                   .ToList();
        }

        /// <summary>
        /// Obtém calibrações dentro de um intervalo de datas.
        /// </summary>
        public IEnumerable<SensorCalibracoes> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            return Find(sc => sc.DataCalibracao >= startDate && sc.DataCalibracao <= endDate)
                   .OrderByDescending(sc => sc.DataCalibracao)
                   .ToList();
        }
    }
}