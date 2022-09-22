namespace Canvas.Core.ModelSpace
{
  public struct ViewModel
  {
    public bool IsMove { get; set; }
    public bool IsShape { get; set; }
    public bool IsControl { get; set; }
    public DataModel? Data { get; set; }
  }
}
