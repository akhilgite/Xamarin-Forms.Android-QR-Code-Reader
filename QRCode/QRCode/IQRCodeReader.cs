using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QRCode
{
    public interface IQRCodeReader
    {
        Task<string> ReadQRCode();
    }
}
