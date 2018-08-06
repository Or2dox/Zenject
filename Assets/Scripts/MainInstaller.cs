using Zenject;

public class MainInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<Circle>().AsSingle();
        Container.Bind<Trajectory>().AsSingle();
        Container.BindInterfacesAndSelfTo<Dispatcher>().AsSingle();
    }
}