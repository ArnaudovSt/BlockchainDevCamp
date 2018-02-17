pragma solidity ^0.4.18;

import './Payable.sol';

contract Withdrawable is Payable {
    event LogWithdraw(address indexed _to, uint256 _amount);

    function withdraw(address _to, uint256 _amount) public onlyOwner {
        require(_to != address(0));
        require(this.balance >= _amount);
        _to.transfer(_amount);
        LogWithdraw(_to, _amount);
    }
}