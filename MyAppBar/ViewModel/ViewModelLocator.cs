/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:MyAppBar.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using Autofac;
using Autofac.Extras.CommonServiceLocator;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using MyAppBar.Contracts;
using MyAppBar.Data;
using MyAppBar.Model;
using MyAppBar.View;
using UtilityLib;
using UtilityLib.Contracts;
using TagDialogWindow = MyAppBar.View.TagDialogWindow;

namespace MyAppBar.ViewModel {
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewModelLocator {
        static ViewModelLocator() {
            var builder = new ContainerBuilder();

            // Register types of this assembly
            // ...
            builder.RegisterType<DisplayManager>().As<IDisplayManager>();

            builder.RegisterType<Category>().As<Category>();
            builder.RegisterType<Link>().As<Link>();
            builder.RegisterType<Tag>().As<Tag>();
            builder.RegisterType<Note>().As<Note>();

            builder.RegisterType<MainWindow>().As<MainWindow>();
            builder.RegisterType<CategoryDialogWindow>().As<CategoryDialogWindow>();
            builder.RegisterType<LinkDialogWindow>().As<LinkDialogWindow>();
            builder.RegisterType<TagDialogWindow>().As<TagDialogWindow>();
            //builder.RegisterType<NoteDialogWindow>().As<NoteDialogWindow>();

            builder.RegisterType<MainViewModel>().As<MainViewModel>();
            builder.RegisterType<CategoryDialogViewModel>().As<CategoryDialogViewModel>();
            builder.RegisterType<LinkDialogViewModel>().As<LinkDialogViewModel>();
            builder.RegisterType<TagDialogViewModel>().As<TagDialogViewModel>();
            //builder.RegisterType<NoteDialogViewModel>().As<NoteDialogViewModel>();


            // Register types based on design mode
            if (ViewModelBase.IsInDesignModeStatic) {
                //builder.RegisterType<DesignDataService>().As<IDataService>();
            } else {
                builder.RegisterType<DataService>().As<IDataService>();
            }

            //if (ServiceLocator.IsLocationProviderSet) {
            //    IContainer container = ServiceLocator.Current.GetInstance<IContainer>();
            //    builder.Update(container);
            //} else {
            IContainer container = builder.Build();

            //var b = new ContainerBuilder();
            //b.RegisterInstance(container).As<IContainer>();
            //b.Update(container);

            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(container));
            //}
        }

        /// <summary>
        /// Ensures that the static constructor has finished and bootstrapping took place. 
        /// </summary>
        public static void Init() {
            
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        /// <summary>
        /// Gets the DialogWindowVM property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
        public CategoryDialogViewModel Categories {
            get {
                return ServiceLocator.Current.GetInstance<CategoryDialogViewModel>();
            }
        }

        /// <summary>
        /// Gets the TagDialogViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
        public TagDialogViewModel Tags {
            get {
                return ServiceLocator.Current.GetInstance<TagDialogViewModel>();
            }
        }

        /// <summary>
        /// Gets the LinkDialogViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
        public LinkDialogViewModel Links {
            get {
                return ServiceLocator.Current.GetInstance<LinkDialogViewModel>();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup() {

        }
    }
}