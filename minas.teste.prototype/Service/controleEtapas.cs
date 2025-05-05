using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;

public class TextBoxTrackBarSynchronizer
{
    private readonly TextBox _textBox;
    private readonly TrackBar _trackBar;
    private readonly int _minimumValue;
    private readonly int _maximumValue;
    public int MaximumValue => _maximumValue;
    public int maxControlEtapas => _trackBar.Value;



    public TextBoxTrackBarSynchronizer(TextBox textBox, TrackBar trackBar, int minimumValue, int maximumValue)
    {
        _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
        _trackBar = trackBar ?? throw new ArgumentNullException(nameof(trackBar));
        _minimumValue = minimumValue;
        _maximumValue = maximumValue;

        _trackBar.Minimum = _minimumValue;
        _trackBar.Maximum = _maximumValue;
        _trackBar.Value = _minimumValue; // Valor inicial

        _textBox.Text = _minimumValue.ToString(); // Texto inicial

        _textBox.KeyPress += TextBox_KeyPress;
        _textBox.TextChanged += TextBox_TextChanged;
        _trackBar.ValueChanged += TrackBar_ValueChanged;
    }

    private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8)
        {
            e.Handled = true;
        }
        else if (char.IsDigit(e.KeyChar))
        {
            int digit = int.Parse(e.KeyChar.ToString());
            if (digit < _minimumValue || digit > _maximumValue)
            {
                e.Handled = true;
            }
        }
    }

    private void TextBox_TextChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_textBox.Text))
        {
            if (int.TryParse(_textBox.Text, out int value))
            {
                if (value >= _minimumValue && value <= _maximumValue)
                {
                    if (_trackBar.Value != value)
                    {
                        _trackBar.Value = value;
                    }
                }
                // Não corrigimos o texto aqui. Deixamos o KeyPress lidar com a entrada.
            }
            else
            {
                _textBox.Text = _minimumValue.ToString();
                _trackBar.Value = _minimumValue;
            }
        }
        else
        {
            _textBox.Text = _minimumValue.ToString();
            _trackBar.Value = _minimumValue;
        }
    }

    private void TrackBar_ValueChanged(object sender, EventArgs e)
    {
        _textBox.Text = _trackBar.Value.ToString();
    }
}