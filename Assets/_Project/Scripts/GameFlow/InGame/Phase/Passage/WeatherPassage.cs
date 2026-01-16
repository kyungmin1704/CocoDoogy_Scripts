using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.GameFlow.InGame.Weather;

namespace CocoDoogy.GameFlow.InGame.Phase.Passage
{
    public class WeatherPassage: PassageBase
    {
        private readonly WeatherType weatherType = WeatherType.Sunny;
        
        
        public WeatherPassage(int actionPoints, WeatherType weatherType)
        {
            ActionPoints = actionPoints;
            this.weatherType = weatherType;
        }
        
        
        public override void Execute()
        {
            CommandManager.Weather(weatherType);
        }
    }
}