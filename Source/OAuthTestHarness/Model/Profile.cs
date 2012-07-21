
namespace OAuthTestHarness.Model
{
    public class Profile
    {
        public string id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public string email { get; set; }
        public string relationship { get; set; }

        public string url { get; set; }
        public string unions { get; set; }
        public bool? is_alive { get; set; }
        public string creator { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }
}
