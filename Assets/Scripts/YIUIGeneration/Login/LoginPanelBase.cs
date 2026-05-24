using System;
using YIUIBind;
using YIUIFramework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace YIUI.Login
{



    /// <summary>
    /// 由YIUI工具自动创建 请勿手动修改
    /// </summary>
    public abstract class LoginPanelBase:BasePanel
    {
        public const string PkgName = "Login";
        public const string ResName = "LoginPanel";
        
        public override EWindowOption WindowOption => EWindowOption.None;
        public override EPanelLayer Layer => EPanelLayer.Panel;
        public override EPanelOption PanelOption => EPanelOption.None;
        public override EPanelStackOption StackOption => EPanelStackOption.VisibleTween;
        public override int Priority => 0;
        public UnityEngine.UI.Button u_ComBtnStart { get; private set; }

        
        protected sealed override void UIBind()
        {
            u_ComBtnStart = ComponentTable.FindComponent<UnityEngine.UI.Button>("u_ComBtnStart");

        }

        protected sealed override void UnUIBind()
        {

        }
     
   
   
    }
}