using System.Text;
using System.Text.RegularExpressions;

namespace FashionStore.Utilities
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string phrase)
        {
            if (string.IsNullOrEmpty(phrase)) return "";

            string str = phrase.ToLower();

            // Xóa dấu tiếng Việt
            str = Regex.Replace(str, @"[áàạảãâấầậẩẫăắằặẳẵ]", "a");
            str = Regex.Replace(str, @"[éèẹẻẽêếềệểễ]", "e");
            str = Regex.Replace(str, @"[óòọỏõôốồộổỗơớờợởỡ]", "o");
            str = Regex.Replace(str, @"[úùụủũưứừựửữ]", "u");
            str = Regex.Replace(str, @"[íìịỉĩ]", "i");
            str = Regex.Replace(str, @"[ýỳỵỷỹ]", "y");
            str = Regex.Replace(str, @"[đ]", "d");

            // Chỉ giữ lại ký tự a-z, 0-9 và dấu gạch ngang
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

            // Chuyển khoảng trắng thành gạch ngang
            str = Regex.Replace(str, @"\s+", "-").Trim();

            return str;
        }
    }
}