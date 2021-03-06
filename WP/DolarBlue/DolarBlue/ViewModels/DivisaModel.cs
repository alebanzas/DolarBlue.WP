﻿using System;
using System.Collections.Generic;

namespace DolarBlue.ViewModels
{
    public class DivisaModel
    {
        public DivisaModel()
        {
            Divisas = new List<DivisaViewModel>();
        }

        public IList<DivisaViewModel> Divisas { get; set; }

        public DateTime Actualizacion { get; set; }

        public string ActualizacionStr
        {
            get { return string.Format("{0} {1}", Actualizacion.ToLongDateString(), Actualizacion.ToLongTimeString()); }
        }
    }
}