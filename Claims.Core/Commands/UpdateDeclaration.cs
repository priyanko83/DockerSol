using Claims.Core.DTOs;
using CQRSFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Claims.Core.Commands
{
    [Serializable]
    public class UpdateDeclaration : ApplicationCommand
    {
        public UpdateDeclaration(DeclarationDTO declaration) : base(CommandType.UpdateDeclaration)
        {
            this.Declaration = declaration;
        }

        public DeclarationDTO Declaration { get; set; }
    }
}
