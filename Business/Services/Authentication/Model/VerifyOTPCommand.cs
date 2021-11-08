﻿using Core.Utilities.Results;
using MediatR;

namespace Business.Services.Authentication.Model
{
    /// <summary>
    ///     It is the command sent to the user for One Time Password control.
    /// </summary>
    public class VerifyOtpCommand : IRequest<IDataResult<DArchToken>>
    {
        /// <summary>
        ///     Specifies the subtype so that the same provider user can enter from different systems.
        /// </summary>
        public string ProviderSubType { get; set; }

        public string Mail { get; set; }
        public int Code { get; set; }
    }
}