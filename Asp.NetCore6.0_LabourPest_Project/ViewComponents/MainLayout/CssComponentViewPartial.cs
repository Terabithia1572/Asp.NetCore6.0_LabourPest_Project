﻿using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.ViewComponents.MainLayout
{
	public class CssComponentViewPartial:ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			return View();
		}
	}
}
