using CocoDoogy.Audio;
using CocoDoogy.GameFlow.InGame.Weather;
using CocoDoogy.WeatherEffect;
using UnityEngine;
using UnityEngine.InputSystem; // 추가!

namespace CocoDoogy.Test
{
public class WeatherEffectTester : MonoBehaviour
{
    [Header("테스트 설정")]
    [SerializeField] private float testDuration = 3f;
    [SerializeField] private SfxType testSfxType = SfxType.None;

    private void Update()
    {
        // New Input System 방식
        if (Keyboard.current[Key.Digit1].wasPressedThisFrame)
            TestWeather(WeatherType.Sunny);
        
        if (Keyboard.current[Key.Digit2].wasPressedThisFrame)
            TestWeather(WeatherType.Rain);
        
        if (Keyboard.current[Key.Digit3].wasPressedThisFrame)
            TestWeather(WeatherType.Snow);
        
        if (Keyboard.current[Key.Digit4].wasPressedThisFrame)
            TestWeather(WeatherType.Mirage);
    }

    private void TestWeather(WeatherType weatherType)
    {
        Debug.Log($"테스트 실행: {weatherType}, Duration: {testDuration}초");
        WeatherEffectManager.PlayEffect(weatherType, testDuration);
    }
}
}