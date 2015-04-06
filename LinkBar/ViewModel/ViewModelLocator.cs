/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:LinkBar.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using Autofac;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using LinkBar.Contracts;
using LinkBar.Data;
using LinkBar.Model;
using LinkBar.View;
using UtilityLib;
using UtilityLib.Contracts;

namespace LinkBar.ViewModel {
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewModelLocator {
        private static readonly IContainer Autofac;

        static ViewModelLocator() {
            // Create the builder with which components/services are registered.
            var builder = new ContainerBuilder();

            // Register types that expose interfaces...
            builder.RegisterType<DisplayManager>().As<IDisplayManager>();
            builder.RegisterType<DataService>().As<IDataServiceAsync>();
            builder.RegisterType<LinkRepositoryAsync>().As<ILinkRepositoryAsync>();

            builder.RegisterType<Category>();
            builder.RegisterType<Tag>();
            builder.RegisterType<Link>();

            builder.RegisterType<MainWindow>();
            builder.RegisterType<CategoryDialogWindow>();
            builder.RegisterType<TagDialogWindow>();
            builder.RegisterType<LinkDialogWindow>();

            builder.RegisterType<MainViewModel>();
            builder.RegisterType<CategoryDialogViewModel>();
            builder.RegisterType<TagDialogViewModel>();
            builder.RegisterType<LinkDialogViewModel>();

            // Build the container to finalize registrations
            // and prepare for object resolution.
            Autofac = builder.Build();
        }

        public static IContainer InitContainer() {
            return Autofac;
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main {
            get { return Autofac.Resolve<MainViewModel>(); }
        }

        /// <summary>
        /// Gets the DialogWindowVM property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
        public CategoryDialogViewModel Categories {
            get {
                return Autofac.Resolve<CategoryDialogViewModel>();
            }
        }

        /// <summary>
        /// Gets the TagDialogViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
        public TagDialogViewModel Tags {
            get {
                return Autofac.Resolve<TagDialogViewModel>();
            }
        }

        /// <summary>
        /// Gets the LinkDialogViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
        public LinkDialogViewModel Links {
            get {
                return Autofac.Resolve<LinkDialogViewModel>();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup() {

        }
    }
}