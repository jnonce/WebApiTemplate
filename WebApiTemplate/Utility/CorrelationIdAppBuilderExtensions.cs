using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Serilog.Context;

namespace webapitmpl.Utility
{
    /// <summary>
    /// Extensions for <see cref="IAppBuilder"/> involving correlation ids
    /// </summary>
    public static class CorrelationIdAppBuilderExtensions
    {
        /// <summary>
        /// Accepts an incoming correlation identifier from an HTTP request header.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="header">The header to read.</param>
        /// <param name="chooseValue">Choose the final correlation id, given the value from the header (or null).</param>
        public static void AcceptCorrelationId(
            this IAppBuilder app, 
            string header, 
            Func<string, string> chooseValue)
        {
            app.Use(
                (ctx, next) =>
                {
                    string id;

                    string[] headerValues;
                    if (ctx.Request.Headers.TryGetValue(header, out headerValues)
                        && headerValues.Length == 1
                        && !String.IsNullOrEmpty(headerValues[0]))
                    {
                        id = chooseValue(headerValues[0]);
                    }
                    else
                    {
                        id = chooseValue(null);
                    }

                    ctx.SetCorrelationId(id);
                    return next();
                });
        }

        /// <summary>
        /// Emits the correlation identifier to a response header.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="header">The header to write.</param>
        public static void EmitCorrelationId(this IAppBuilder app, string header)
        {
            Action<object> appendHeader =
                obj =>
                {
                    var ctx = (IOwinContext)obj;
                    string id = ctx.GetCorrelationId();
                    if (id != null)
                    {
                        ctx.Response.Headers.Set(header, id);
                    }
                };

            app.Use(
                (ctx, next) =>
                {
                    ctx.Response.OnSendingHeaders(appendHeader, ctx);
                    return next();
                });
        }

        /// <summary>
        /// Logs the correlation identifier.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="propertyName">Name of the property.</param>
        public static void LogCorrelationId(this IAppBuilder app, string propertyName)
        {
            app.Use(
                async (ctx, next) =>
                {
                    string id = ctx.GetCorrelationId();
                    if (id != null)
                    {
                        using (LogContext.PushProperty(propertyName, id, false))
                        {
                            await next();
                        }
                    }
                    else
                    {
                        await next();
                    }
                });
        }
    }
}
