var ConvertLib = artifacts.require("./ConvertLib.sol");
var MetaCoin = artifacts.require("./MetaCoin.sol");
var MetaCoinStorage = artifacts.require("../contracts/MetaCoinStorage.sol");
let db;

module.exports = (deployer) => {
  deployer.then(async () => {
    await deployer.deploy(MetaCoinStorage);
    db = await MetaCoinStorage.deployed();
    await deployer.deploy(ConvertLib);
    deployer.link(ConvertLib, MetaCoin);
    await deployer.deploy(MetaCoin, db.address);
    db.modifyAccess(MetaCoin.address, true);
  })
};
