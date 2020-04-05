public class Result
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Image { get; set; }
    public Result Inner { get; set; }

    public Result(
        string id = null,
        string name = null,
        string image = null,
        Result inner = null)
    {
        Id = id;
        Name = name;
        Image = image;
        Inner = inner;
    }
}