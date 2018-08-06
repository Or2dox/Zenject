using Zenject;

//[CreateAssetMenu(fileName = "SettingsInstaller", menuName = "Installers/SettingsInstaller")]
public class SettingsInstaller : ScriptableObjectInstaller<SettingsInstaller>
{
    public Settings settings;

    public override void InstallBindings()
    {
        Container.BindInstances(settings);
    }
}