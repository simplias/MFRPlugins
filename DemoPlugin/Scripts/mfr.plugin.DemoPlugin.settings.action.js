mfr.plugin.DemoPlugin = mfr.plugin.DemoPlugin || {};
mfr.plugin.DemoPlugin.settings = mfr.plugin.DemoPlugin.settings || {};

mfr.plugin.DemoPlugin.settings.action = function (context, titleCallback) {

    var pluginContext = mfr.plugin.DemoPlugin.pluginContext;

    titleCallback(pluginContext.displayName, [mfr.breadcrumb.administration()]);

    var model = new mfr.plugin.DemoPlugin.settings.model();
    var view = pluginContext.registerView(mfr.plugin.DemoPlugin.settings.view, model);
    view.render();

    model.fetch();
};
