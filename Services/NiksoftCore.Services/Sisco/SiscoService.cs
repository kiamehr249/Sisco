using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.Services.Sisco
{
    public interface ISiscoService
    {
        string GetNormalPartyNumber(string originalNum, string prfix = "");
    }

    public class SiscoService : ISiscoService
    {
        public SiscoService()
        {

        }

        public string GetNormalPartyNumber(string originalNum, string prfix = "")
        {
            string[] starts = new string[] { "1", "2", "3", "4", "5", "6", "7", "8" };
            string normalNum = null;
            if (string.IsNullOrEmpty(originalNum) || originalNum.StartsWith("b00"))
            {
                return null;
            }

            if (!string.IsNullOrEmpty(prfix))
            {
                originalNum = originalNum.Substring(prfix.Length);
            }

            if (originalNum.StartsWith("021") && originalNum.Length == 11)
            {
                return originalNum.Substring(3);
            }
            else if (originalNum.StartsWith("21") && originalNum.Length == 10)
            {
                return originalNum.Substring(2);
            }
            else if (originalNum.StartsWith("98") && originalNum.Length == 10)
            {
                return originalNum.Substring(2);
            }
            else if (originalNum.StartsWith("021") && originalNum.Length == 11)
            {
                return originalNum.Substring(3);
            }
            else if (originalNum.StartsWith("+98") && originalNum.Length == 11)
            {
                return originalNum.Substring(3);
            }
            else if (originalNum.StartsWith("98") && starts.Contains(originalNum.Substring(2, 1)) && originalNum.Length == 12)
            {
                return "0" + originalNum.Substring(2);
            }
            else if (originalNum.StartsWith("+98") && starts.Contains(originalNum.Substring(3, 1)) && originalNum.Length == 13)
            {
                return "0" + originalNum.Substring(3);
            }
            else if (starts.Contains(originalNum.Substring(0, 1)) && originalNum.Length == 10)
            {
                return "0" + originalNum;
            }
            else if (originalNum.StartsWith("9") && originalNum.Length == 10)
            {
                return "0" + originalNum;
            }
            else if (originalNum.StartsWith("989") && originalNum.Length == 12)
            {
                return "0" + originalNum.Substring(2);
            }
            else if (originalNum.StartsWith("+989") && originalNum.Length == 13)
            {
                return "0" + originalNum.Substring(3);
            }
            else if (originalNum.StartsWith("+") && originalNum.Substring(1, 2) != "98")
            {
                return "00" + originalNum.Substring(1);
            }

            return normalNum;
        }
    }
}
