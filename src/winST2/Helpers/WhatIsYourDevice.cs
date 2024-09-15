using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Controls;

namespace Wpf.Ui.Demo.Mvvm.Helpers;

internal class WhatIsYourDevice
{
    public static SymbolIcon IconSelector(string name)
    {
        if (name.Contains("light", StringComparison.CurrentCultureIgnoreCase) || name.Contains("bulb", StringComparison.CurrentCultureIgnoreCase))
        {
            return new SymbolIcon(SymbolRegular.Lightbulb24);
        }
        else if (name.Contains("plug", StringComparison.CurrentCultureIgnoreCase) || name.Contains("outlet", StringComparison.CurrentCultureIgnoreCase) || name.Contains("power", StringComparison.CurrentCultureIgnoreCase))
        {
            return new SymbolIcon(SymbolRegular.PlugConnected24);
        }
        else if (name.Contains("switch", StringComparison.CurrentCultureIgnoreCase))
        {
            return new SymbolIcon(SymbolRegular.ToggleMultiple24);
        }
        else if (name.Contains("contact", StringComparison.CurrentCultureIgnoreCase))
        {
            return new SymbolIcon(SymbolRegular.Door20);
        }
        else if (name.Contains("sensor", StringComparison.CurrentCultureIgnoreCase) || name.Contains("people", StringComparison.CurrentCultureIgnoreCase) || name.Contains("motion", StringComparison.CurrentCultureIgnoreCase))
        {
            return new SymbolIcon(SymbolRegular.PersonAlert24);
        }
        else if (name.Contains("temp", StringComparison.CurrentCultureIgnoreCase) || name.Contains("humidity", StringComparison.CurrentCultureIgnoreCase))
        {
            return new SymbolIcon(SymbolRegular.Temperature24);
        }
        else if (name.Contains("tv", StringComparison.CurrentCultureIgnoreCase))
        {
            return new SymbolIcon(SymbolRegular.Tv24);
        }
        else if (name.Contains("pc", StringComparison.CurrentCultureIgnoreCase))
        {
            return new SymbolIcon(SymbolRegular.Desktop24);
        }
        else if (name.Contains("에어컨", StringComparison.CurrentCultureIgnoreCase))
        {
            return new SymbolIcon(SymbolRegular.WeatherSnowflake24);
        }
        else
        {
            return new SymbolIcon(SymbolRegular.QuestionCircle24);
        }
    }
}
