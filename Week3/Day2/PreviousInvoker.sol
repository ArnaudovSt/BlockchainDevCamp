pragma solidity ^0.4.18;

contract InvokersReport {
    struct Successor {
        address nextInvoker;
        bytes12 workLog;
        uint256 completionTime;
    }

    mapping(address => Successor) public topology;
    address public lastInvoker;

    function InvokersReport() public {
        topology[address(0)] = Successor(msg.sender, "Initialized", now);
    }

    function jobComplete(address _successor, bytes12 _workLog) public {
        require(topology[lastInvoker].nextInvoker == msg.sender);
        require(_successor != address(0));
        require(topology[_successor].nextInvoker == address(0));
        
        lastInvoker = msg.sender;
        topology[lastInvoker] = Successor(_successor, _workLog, now);
    }
}