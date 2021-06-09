﻿using SiraUtil.Services.Controllers;
using SiraUtil.Tools.FPFC;
using SiraUtil.Tools.SongControl;
using Zenject;

namespace SiraUtil.Installers
{
    internal class SiraPlayerInstaller : Installer
    {
        public override void InstallBindings()
        {
            // FPFC stuff
            Container.BindInterfacesAndSelfTo<GameMenuControllerAccessor>().AsSingle();
            Container.BindInterfacesTo<GameTransformFPFCListener>().AsSingle();

            // SongControl stuff
            Container.BindInterfacesTo<SongControlManager>().AsSingle();
        }
    }
}