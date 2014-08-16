mfr.plugin.DemoPlugin = mfr.plugin.DemoPlugin || {};
mfr.plugin.DemoPlugin.settings = mfr.plugin.DemoPlugin.settings || {};

mfr.plugin.DemoPlugin.settings.model = mfr.model.Base.extend({
    url: function () {
        return mfr.plugin.DemoPlugin.pluginContext.getServerUrl() + "Settings";
    },
});
