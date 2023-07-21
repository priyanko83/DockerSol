using Claims.Core.DTOs;
using CQRSFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Claims.Core.Commands
{
    [Serializable]
    public class CreateNewDeclaration : ApplicationCommand
    {
        public CreateNewDeclaration(DeclarationDTO declaration) : base(CommandType.CreateNewDeclaration)
        {
            this.Declaration = declaration;
        }

        public DeclarationDTO Declaration { get; set; }
    }
}
