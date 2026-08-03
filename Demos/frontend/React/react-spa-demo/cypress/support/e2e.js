import "@cypress/code-coverage/support";
import compareSnapshotCommand from "cypress-image-diff-js";

import "./commands";

compareSnapshotCommand();