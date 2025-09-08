using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Models;

namespace SSRAG.Services
{
    public partial interface IFormManager
    {   
        Task SendNotifyAsync(Form form, List<TableStyle> styles, FormData data);
    }
}
