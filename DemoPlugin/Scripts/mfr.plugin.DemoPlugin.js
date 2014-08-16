
$(mfr.plugin.DemoPlugin.pluginContext).bind("activate", function() {
    mfr.plugin.DemoPlugin.pluginContext.registerSettings({
        name: "DemoPlugin",
        action: mfr.plugin.DemoPlugin.settings.action
    });
});


