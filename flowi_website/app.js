express = require('express');
app = express();

app.set('view engine', 'ejs');

var path = require('path');
var bodyParser = require('body-parser');
var leaders = new Array(3);

app.use(bodyParser.urlencoded({ extended: false}));
app.use(express.static(path.join(__dirname, 'views/public')));

app.get('/', function (req, res) {
    res.render('index', {leaders: leaders});
});

app.post('/', function(req, res) {
    temp = {"name": req.body.name,
            "score": parseInt(req.body.score)}
     leaders.push(temp)
     leaders = leaders.sort((x, y) => y.score - x.score).slice(0,3);
     res.redirect("/");
            
});

app.listen(8000, function () {
  console.log('Listening to port: 8000');
});