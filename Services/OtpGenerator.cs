namespace NetCoreIntermediate.Services
{
    public class OtpGenerator
    {
        public static string GenerateUniqueDigitOTP(int length)
        {
            if (length > 10)
                throw new ArgumentException("OTP length cannot be greater than 10 when digits must be unique.");

            Random random = new Random();
            // Generate a list of digits from 0 to 9
            var digits = Enumerable.Range(0, 10).ToList();

            // Shuffle the digits randomly and take the required number
            var otp = string.Concat(digits.OrderBy(x => random.Next()).Take(length));
            return otp;
        }
    }
}
