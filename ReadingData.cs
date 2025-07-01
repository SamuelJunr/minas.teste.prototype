// Update the ReadingData class definition to include the FormattingType property
public class ReadingData
{
    public string Id { get; set; }
    public string Name { get; set; }
    public double Value { get; set; }
    public string Type { get; set; }
    public string OriginalUnit { get; set; }
    public string DisplayUnit { get; set; }
    public string ValueTextBoxName { get; set; }
    public string UnitLabelName { get; set; }
    public string NameLabelText { get; set; }
    public string FormattingType { get; set; } // Added property
    public ReadingData Clone();
}
