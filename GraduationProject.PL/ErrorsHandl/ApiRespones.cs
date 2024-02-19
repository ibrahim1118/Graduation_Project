namespace GraduationProject.API.ErrorsHandl
{
    public class ApiRespones
    {
        public int stustseCode { get; set; }
        public string? Message { get; set; }

        public ApiRespones(int stustsecode, string? Message = null)
        {
            stustseCode = stustsecode;
            this.Message = Message ?? GetErorrMessage(stustsecode);
        }

        private string? GetErorrMessage(int v)
        {
            return stustseCode switch
            {
                400 => "Bad Requset",
                404 => "Not Found",
                401 => "Authorized , you Are Not",
                500 => "Server Erorr",
                _ => null

            };
        }

        public override string ToString()
        {
            return $"{stustseCode}\n{Message}";
        }
    }
}
