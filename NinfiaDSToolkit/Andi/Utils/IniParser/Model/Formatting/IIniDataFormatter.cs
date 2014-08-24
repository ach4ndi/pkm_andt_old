using NinfiaDSToolkit.Andi.Utils.IniParser.Model.Configuration;

namespace NinfiaDSToolkit.Andi.Utils.IniParser.Model.Formatting
{
    /// <summary>
    ///     Formats a IniData structure to an string
    /// </summary>
    public interface IIniDataFormatter
    {
        /// <summary>
        ///     Produces an string given
        /// </summary>
        /// <returns>The data to string.</returns>
        /// <param name="iniData">Ini data.</param>
        string IniDataToString(IniData iniData);

        /// <summary>
        ///     Configuration used by this formatter when converting IniData
        ///     to an string
        /// </summary>
        IIniParserConfiguration Configuration {get;set;}
    }
    
} 