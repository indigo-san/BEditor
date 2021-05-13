﻿using System;
using System.Threading;

using BEditor.Data;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BEditor
{

    /// <summary>
    /// Represents an application.
    /// </summary>
    public interface IApplication : IParentSingle<Project?>
    {
        /// <summary>
        /// Gets or sets the status of an application.
        /// </summary>
        public Status AppStatus { get; set; }

        /// <summary>
        /// Gets the <see cref="IServiceCollection"/>.
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        /// Gets the <see cref="ILoggerFactory"/>.
        /// </summary>
        public ILoggerFactory LoggingFactory { get; }

        /// <summary>
        /// Gets the <see cref="SynchronizationContext"/>.
        /// </summary>
        public SynchronizationContext UIThread { get; }

        /// <summary>
        /// Occurs when the project is opened.
        /// </summary>
        public event EventHandler<ProjectOpenedEventArgs>? ProjectOpened;

        /// <summary>
        /// Restore the application configuration.
        /// </summary>
        /// <param name="project">The project to restore the config.</param>
        /// <param name="directory">The directory of config.</param>
        public void RestoreAppConfig(Project project, string directory);

        /// <summary>
        /// Save the application configuration.
        /// </summary>
        /// <param name="project">The project to save the config.</param>
        /// <param name="directory">The directory of config.</param>
        public void SaveAppConfig(Project project, string directory);
    }
}