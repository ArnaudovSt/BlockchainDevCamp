pragma solidity ^0.4.18;

contract HoneyPot {
    mapping (address=>uint256) public balances;

    function put() public payable {
        balances[msg.sender] += msg.value;
    }

    function get() public {
        if (!msg.sender.call.value(balances[msg.sender])()) {
            revert();
        }

        balances[msg.sender] = 0;
    }
}