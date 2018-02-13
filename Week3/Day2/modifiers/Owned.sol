pragma solidity ^0.4.18;

contract Owned {
    event OwnerChanged(address indexed _oldOwner, address _newOwner);

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
        OwnerChanged(owner, _newOwner);
        owner = _newOwner;
    }
}
