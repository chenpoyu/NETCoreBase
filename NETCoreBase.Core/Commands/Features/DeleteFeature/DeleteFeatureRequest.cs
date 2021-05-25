using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace NETCoreBase.Core.Commands.Features
{
    public class DeleteFeatureRequest : IRequest<Unit>
    {
        /// <summary>
        /// 功能Id
        /// </summary>
        public List<Guid> Id { get; set; }
    }
}
