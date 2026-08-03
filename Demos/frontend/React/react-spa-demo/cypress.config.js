import { defineConfig } from "cypress";
import registerCodeCoverage from "@cypress/code-coverage/task";
import getCompareSnapshotsPlugin from "cypress-image-diff-js/plugin";

export default defineConfig({
    allowCypressEnv: false,
    e2e: {
        baseUrl: "http://localhost:5173",
        supportFile: "cypress/support/e2e.js",
        setupNodeEvents(on, config) {
            on("task", {
                Log(message) {
                    console.log(`[spec] ${message}`);
                    return null;
                }
            });

            registerCodeCoverage(on, config);
            getCompareSnapshotsPlugin(on, config);

            return config;
        }
    },
    component: {
        devServer: {
            framework: "react",
            bundler: "vite",
        }
    },
})