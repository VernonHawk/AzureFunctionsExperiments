namespace ServiceGlue
{
    public class Post
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Body { get; set; }
        public int? Rating { get; set; }

        public override string ToString() =>
            $"Id:{Id}, Title:{Title}, Author:{Author}, Body:{Body}, Rating:{Rating}";
    }
}