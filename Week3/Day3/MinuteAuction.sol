pragma solidity ^0.4.18;

import './common/SafeMath.sol';
import './Withdrawable.sol';

interface IBalanceHolder {
    function addToBalance(address _owner, uint256 _amountToAdd) public;
}

contract MinuteAuction is Withdrawable {
    using SafeMath for uint256;

    uint256 public end = now.add(1 minutes);
    
    IBalanceHolder public balanceHolder;
    
    uint256 public totalSupply;
    uint256 public price;

    event LogPurchase(address indexed _buyer, uint256 _amount);

    function MinuteAuction(address _balanceHolderAddress, uint256 _totalSupply, uint256 _priceInWei) public {
        require(_balanceHolderAddress != address(0));
        balanceHolder = IBalanceHolder(_balanceHolderAddress);
        totalSupply = _totalSupply;
        price = _priceInWei;
    }

    function buy() public payable {
        require(now <= end);
        uint256 sellAmount = msg.value.mul(1 ether);
        sellAmount = sellAmount.div(price);
        totalSupply = totalSupply.sub(sellAmount);
        balanceHolder.addToBalance(msg.sender, sellAmount);
        LogPurchase(msg.sender, sellAmount);
    }
}