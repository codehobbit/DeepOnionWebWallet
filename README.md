# DeepOnion Web Wallet

DeepOnion Web Wallet is the source that is used to run www.onionwallet.ch.

## Getting Started

If you want to run the project you will need to have gt Wallet installed, SQL Server (Express is fine) installed and also IIS with asp.net installed. Obviously you will need Visual Studio.

### Prepare Onion Wallet 

In Order to run the code you must have installed DeepOnion Wallet (download it from deeponion.org). 
Open DeepOnion.conf in %AppData%/DeepOnion and set these values:
```
listen=1
server=1
txindex=1
rpcuser=SomeUserName
rpcpassword=SomePassword
rpcport=18580
rpcallowport=18580
rpcconnect=127.0.0.1
rpcallowip=*
daemon=1
```
Replace SomeUserName and SomePassword with whatever you feel like.. 

Make sure to create an account for the Donations. Run command 'getaccountaddress Donations' in Wallet command line.

### Prepare your local IIS 

Create a new Website on your local IIS. Point it to the WWWOnionWallet - Path and let it listen to port 89. You can also switch to IIS Express in WWWOnionWallet - Properties.

### Prepare MS SQL Server

Create a new database and create tables. You can find the necessary tables when you doubleclick on OnionWallet.Data/OnionModel.edmx.

### Prepare the solution

Rename WWWOnionWallet/Web.config.example to Web.config and adjust the AppSettings, also adjust the connection string for the database.
Rename OnionWallet.Data/App.config.example to App.config and adjust the connection string.

### Run

You should be pretty good to go now...

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Thanks to BitcoinLib, used part of their source, good work.
* https://deeponion.org/community/threads/deeponion-web-wallet-onionwallet-ch.2308/

