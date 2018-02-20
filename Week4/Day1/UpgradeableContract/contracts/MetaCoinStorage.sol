pragma solidity ^0.4.18;

contract MetaCoinStorage {
    mapping (address => uint256) public balances;
    mapping (address => bool) public accessAllowed;

    function MetaCoinStorage() public {
        accessAllowed[msg.sender] = true;
        balances[tx.origin] = 10000;
    }

    modifier fromPlatform() {
        require(accessAllowed[msg.sender] == true);
        _;
    }

    function modifyAccess(address _adr, bool _hasAccess) public fromPlatform {
        accessAllowed[_adr] = _hasAccess;
    }

    function setBalance(address _adr, uint256 _balance) public fromPlatform {
        balances[_adr] = _balance;
    }
}