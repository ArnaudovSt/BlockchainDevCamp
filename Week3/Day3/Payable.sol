pragma solidity ^0.4.18;

import './common/Destructible.sol';

contract Payable is Destructible {
    function () public payable { }

    function contractBalance() public view onlyOwner returns (uint256) {
        return this.balance;
    }
}