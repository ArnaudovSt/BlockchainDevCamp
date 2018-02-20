const SimpleToken = artifacts.require("../contracts/SimpleToken.sol");

const timeTravel = require("../node_modules/zeppelin-solidity/test/helpers/increaseTime");

contract("SimpleToken", ([owner, anotherAccount]) => {
    let sut;

    const name = "Simple Token";
    const symbol = "ST";
    const decimals = "18";
    
    beforeEach(async () => {
        sut = await SimpleToken.new();
    });

    it("Should set owner correctly", async () => {
        let expectedOwner = await sut.owner();

        assert.strictEqual(owner, expectedOwner, "The expected owner is not set");
    });

    it("Should have no totalSupply", async () => {
        let totalSupply = await sut.totalSupply();

        assert.strictEqual("0", totalSupply.toString(10));
    });

    it("Should set the name correctly", async () => {
        let expectedName = await sut.name();

        assert.strictEqual(name, expectedName);
    });

    it("Should set the symbol correctly", async () => {
        let expectedSymbol = await sut.symbol();

        assert.strictEqual(symbol, expectedSymbol);
    });

    it("Should set the decimals correctly", async () => {
        let expectedDecimals = await sut.decimals();

        assert.strictEqual(decimals, expectedDecimals.toString(10));
    });

    it("Can timetravel", async () => {
        const timeBefore = await web3.eth.getBlock("latest").timestamp;
        const travelTime = 86400;
        await timeTravel(travelTime);
        const timeAfter = await web3.eth.getBlock("latest").timestamp;

        assert.isAtLeast(timeAfter - timeBefore, travelTime);
    });
}); 