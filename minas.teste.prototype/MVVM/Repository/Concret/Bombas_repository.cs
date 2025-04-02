using System;
using System.Collections.Generic;
using System.Linq;
using minas.teste.prototype.MVVM.Model.Concrete;

namespace minas.teste.prototype.MVVM.Repository.Concret
{
    public class Pilotagem_bombaRepository : Repository<Pilotagem_bomba>
    {
        public Pilotagem_bombaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<Pilotagem_bomba> GetByValve(int valveId)
        {
            return Find(p => p.ValveId == valveId)
                   .OrderByDescending(p => p.Timestamp)
                   .ToList();
        }

        public IEnumerable<Pilotagem_bomba> GetByPressureRange(double minPsi, double maxPsi)
        {
            return Find(p => p.Psi >= minPsi && p.Psi <= maxPsi);
        }

        public IEnumerable<Pilotagem_bomba> GetEfficientSystems(double minEfficiency)
        {
            return Find(p => p.CalculateEnergyEfficiency() >= minEfficiency);
        }
    }

    public class Pressao_bombaRepository : Repository<Pressao_bomba>
    {
        public Pressao_bombaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<Pressao_bomba> GetByLocation(string location)
        {
            return Find(p => p.Location == location)
                   .OrderByDescending(p => p.Timestamp)
                   .ToList();
        }

        public IEnumerable<Pressao_bomba> GetCriticalPressures()
        {
            return Find(p => p.IsCriticalPressure());
        }

        public IEnumerable<Pressao_bomba> GetByPressureUnit(bool isBar)
        {
            return Find(p => (isBar && p.Bar > 0) || (!isBar && p.Psi > 0));
        }
    }

    public class Rotacao_bombaRepository : Repository<Rotacao_bomba>
    {
        public Rotacao_bombaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<Rotacao_bomba> GetByMotor(string motorId)
        {
            return Find(r => r.MotorId == motorId)
                   .OrderByDescending(r => r.Timestamp)
                   .ToList();
        }

        public IEnumerable<Rotacao_bomba> GetByRotationRange(double minRPM, double maxRPM)
        {
            return Find(r => r.RPM >= minRPM && r.RPM <= maxRPM);
        }

        public IEnumerable<Rotacao_bomba> GetHighPowerSystems(double minPower, double torque)
        {
            return Find(r => r.CalculatePower(torque) >= minPower);
        }
    }

    public class Temperatura_bombaRepository : Repository<Temperatura_bomba>
    {
        public Temperatura_bombaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<Temperatura_bomba> GetCriticalTemperatures()
        {
            return Find(t => t.CheckForExtremeConditions() != "Temperatura OK");
        }

        public IEnumerable<Temperatura_bomba> GetByTemperatureRange(double minTemp, double maxTemp)
        {
            return Find(t => t.Celsius >= minTemp && t.Celsius <= maxTemp);
        }

        public IEnumerable<Temperatura_bomba> GetWithExpansionRisk(double coefficient, double initialLength)
        {
            return Find(t => t.CalculateThermalExpansion(initialLength, coefficient) > 0.1);
        }
    }

    public class Vazao_bombaRepository : Repository<Vazao_bomba>
    {
        public Vazao_bombaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<Vazao_bomba> GetByPumpModel(string model)
        {
            return Find(v => v.PumpModel == model)
                   .OrderByDescending(v => v.Timestamp)
                   .ToList();
        }

        public IEnumerable<Vazao_bomba> GetByFlowStatus(string status)
        {
            return Find(v => v.GetFlowStatus() == status);
        }

        public IEnumerable<Vazao_bomba> GetByFlowRange(double minFlow, double maxFlow, bool isGpm = false)
        {
            return isGpm
                ? Find(v => v.Gpm >= minFlow && v.Gpm <= maxFlow)
                : Find(v => v.Lpm >= minFlow && v.Lpm <= maxFlow);
        }
    }

    public class Dreno_bombaRepository : Repository<Dreno_bomba>
    {
        public Dreno_bombaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<Dreno_bomba> GetByDiameterRange(double minDiam, double maxDiam)
        {
            return Find(d => d.Diameter >= minDiam && d.Diameter <= maxDiam);
        }

        public IEnumerable<Dreno_bomba> GetByMaterial(string material)
        {
            return Find(d => d.Material == material)
                   .OrderByDescending(d => d.Timestamp)
                   .ToList();
        }
    }
}