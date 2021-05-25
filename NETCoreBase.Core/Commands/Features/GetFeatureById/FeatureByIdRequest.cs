using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NETCoreBase.Common;
using MediatR;

namespace NETCoreBase.Core.Commands.Features
{
    public class FeatureByIdRequest : IRequest<FeatureByIdResponse>
    {
        /// <summary>
        /// 功能Id
        /// </summary>
        public Guid Id { get; set; }
    }
}
