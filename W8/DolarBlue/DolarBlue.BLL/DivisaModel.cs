using System;
using System.Collections.Generic;

namespace DolarBlue.BLL
{
	public class DivisaModel
	{
		public DivisaModel()
		{
			Divisas = new List<DivisaViewModel>();
		}

		public IList<DivisaViewModel> Divisas { get; set; }

		public DateTime Actualizacion { get; set; }
	}
}
