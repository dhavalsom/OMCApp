using OMC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMC.BL.Interface
{
    public interface IPatientProblem
    {
        IEnumerable<UserDetails> GetDoctersFromProblem(string problem);
    }
}
