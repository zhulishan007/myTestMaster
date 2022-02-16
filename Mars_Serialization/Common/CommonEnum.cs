using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mars_Serialization.Common
{
    public class CommonEnum
    {
        public enum MarsRecordStatus
        {
            en_None = 0x00,
            en_fromDb,
            en_NewToDb,
            en_ModifiedToDb,
            en_DeletedToDb,
            en_FailedWhenUpdateDB = -0x1
        }
    }
}
