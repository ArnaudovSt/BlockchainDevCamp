const Voting = artifacts.require("../contracts/Voting.sol");

module.exports = (deployer) => {
    deployer.deploy(Voting);
};
