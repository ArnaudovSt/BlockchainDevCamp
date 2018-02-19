pragma solidity ^0.4.18;

contract Owned {
    event LogOwnerChanged(address indexed _oldOwner, address _newOwner);

    address public owner;

    function Owned() public {
        owner = msg.sender;
    }

    modifier onlyOwner() {
        require(msg.sender == owner);
        _;
    }

    function setOwner(address _newOwner) public onlyOwner {
        require(_newOwner != address(0));
        LogOwnerChanged(owner, _newOwner);
        owner = _newOwner;
    }
}
