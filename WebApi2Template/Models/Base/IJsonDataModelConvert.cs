﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi2Template.Models.Base
{
    
    public interface IJsonDataModelConvert<J>
    {
        J ToJsonModel();
    }
}