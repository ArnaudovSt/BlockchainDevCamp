const DocumentRegistry = artifacts.require("../contracts/DocumentRegistry.sol");

module.exports = (deployer) => {
    deployer.deploy(DocumentRegistry);
};
