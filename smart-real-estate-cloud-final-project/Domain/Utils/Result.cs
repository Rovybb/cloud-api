namespace Domain.Utils
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; } 

        protected Result(bool isSuccess, string errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public static Result Success()
        {
            return new Result(true, "");
        }

        public static Result Failure(string errorMessage = "Unknown error occured!")
        {
            return new Result(false, errorMessage);
        }
    }
}